import { error } from '@sveltejs/kit';
import setCookie from 'set-cookie-parser';

function copyCookies(response, event) {
	const cookies = setCookie.parse(response);
	for (const cookie of cookies) {
		event.cookies.set(cookie.name, cookie.value, cookie);
	}
}

export const actions = {
	login: async (event) => {
		const formData = Object.fromEntries(await event.request.formData());
		const response = await fetch('http://webapi:5273/api/Authentication/LogIn', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(formData)
		});
		if (!response.ok) {
			if (response.status === 401) {
				return { ...formData, error: 'Invalid username or password' };
			} else {
				throw error(500, 'Error logging in');
			}
		} else {
			console.log(`Login successful for user ${formData.userName}`);
			copyCookies(response, event);
			event.cookies.set('webapiDemoUserName', formData.userName, { path: '/' });
		}
	},
	logout: async (event) => {
		const response = await fetch('http://webapi:5273/api/Authentication/LogOut', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			}
		});
		if (!response.ok) {
			throw error(500, 'Error logging out');
		} else {
			console.log(`Logout successful for user ${event.cookies.get('xafDemoUserName')}`);
			copyCookies(response, event);
			// Remove our own cookie
			event.cookies.set('webapiDemoUserName', '', { path: '/' });
		}
	}
};
