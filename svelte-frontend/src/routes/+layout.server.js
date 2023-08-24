export function load({ cookies }) {
	const userName = cookies.get('webapiDemoUserName');
	const jwt = cookies.get('webapiDemoJwt');
	return { userName, hasJwt: !!jwt };
}
