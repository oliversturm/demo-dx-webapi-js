export function load({ cookies }) {
	const userName = cookies.get('webapiDemoUserName');
	return { userName };
}
