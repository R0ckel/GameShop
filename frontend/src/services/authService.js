import {userManagementApiUrl} from "../variables/connectionVariables";
import axios from 'axios';
import Cookies from 'js-cookie';
import {setUserData} from "../context/store";

export const authService = {
	register: async (values) => {
		const response = await axios.post(`${userManagementApiUrl}/register`, values);
		return response.data;
	},

	login: async (values) => {
		const response = await axios.post(`${userManagementApiUrl}/login`, values, { withCredentials: true });
		return response.data;
	},

	//delete auth cookies
	logout: async () =>{
		const response = await axios.post(`${userManagementApiUrl}/logout`, {}, { withCredentials: true });
		return response.data;
	},

	getUserClaims: () => {
		//read from cookies
		let headerAndPayload = Cookies.get('headerAndPayload');

		if (headerAndPayload == null) return null;
		//split into parts
		let parts = headerAndPayload.split('.');
		//let header = parts[0];
		let payload = parts[1];

		//get info object
		let payloadStr = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
		return JSON.parse(payloadStr);
	},

	applyUserDataToContext: (userData, dispatch) => {
		const userId = userData["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]
		const username = userData["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"]
		const role = userData["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
		const email = userData["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] ?? ""

		if (userId != null && username != null && role != null && email != null){
			dispatch(setUserData({isLoggedIn: true, userId, username, role, email}));
			return;
		}
		dispatch(setUserData({isLoggedIn: false}))
	}
}