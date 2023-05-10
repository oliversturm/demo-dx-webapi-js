import queryBuilder from 'odata-query';
import { goto } from '$app/navigation';
import { pickBy } from 'lodash';

export async function loadData(fetch, currentUrl, shortTypeName, longTypeName) {
	let sort = currentUrl.searchParams.get('sort');
	let desc = currentUrl.searchParams.get('desc') === 'true';
	const filterParam = currentUrl.searchParams.get('filters');
	let filters = filterParam ? JSON.parse(decodeURIComponent(filterParam)) : {};

	const params = {};
	if (sort) params.orderBy = `${sort} ${desc ? 'desc' : 'asc'}`;

	const filter = {
		and: []
	};
	for (const f of Object.keys(filters)) {
		if (filters[f].filter === 'string' && filters[f].value) {
			filter.and.push({ [f]: { contains: filters[f].value } });
		}
	}
	if (filter.and.length > 0) params.filter = filter;

	const queryString = queryBuilder(params).toString();

	const odataUrl = `http://webapi:5273/api/odata/${shortTypeName}`;
	const fetchUrl = `${odataUrl}${queryString}`;

	const dataSource = fetch(fetchUrl, { redirect: 'manual' })
		.then((res) => {
			if (res.ok) return res;
			throw new Error(`HTTP error, status: ${res.status}`);
		})
		.then((res) => res.json())
		.then((res) => res.value);

	const schemaUrl = `/api/schema/${longTypeName}`;
	const schema = fetch(schemaUrl)
		.then((res) => {
			if (res.ok) return res;
			throw new Error(`HTTP error, status: ${res.status}`);
		})
		.then((res) => res.json());

	return await Promise.all([dataSource, schema])
		.then(([ds, sc]) => ({ dataSource: ds, schema: sc, displayState: { sort, desc, filters } }))
		.catch((err) => {
			console.log(err);
			return { error: err.message };
		});
}

const displayStateQueryString = (s) =>
	new URLSearchParams({
		...pickBy(s), // only those properties that contain something
		filters: encodeURIComponent(JSON.stringify(s.filters))
	}).toString();

export const displayStateChanged =
	(path) =>
	({ detail: state }) => {
		goto(`${path}?${displayStateQueryString(state)}`);
	};
