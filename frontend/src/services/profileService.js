import {userImagesApiUrl, userManagementApiUrl} from "../variables/connectionVariables";
import axios from 'axios';
import {message} from "antd";

export const profileService = {
	getUser: async (id) => {
		try {
			const response = await axios.get(`${userManagementApiUrl}/${id}`);
			return response.data;
		} catch (error) {
			message.error(error.message)
		}
	},
	updateUser: async (form) => {
		try {
			const response = await axios.put(`${userManagementApiUrl}/${form.id}`, form, { withCredentials: true });
			return response.data;
		} catch (error) {
			message.error(error.message)
		}
	},

	putImage: async (id, file) => {
		const formData = new FormData();
		formData.append('file', file);
		console.log(formData.get('file'));
		try {
			await axios.put(`${userImagesApiUrl}/${id}`, formData, {
				headers: {
					'Content-Type': 'multipart/form-data',
				},
				withCredentials: true
			});
		} catch (error) {
			console.log(error.response.data);
			message.error(error.message);
		}
	},
	deleteImage: async (id) => {
		try {
			await axios.delete(`${userImagesApiUrl}/${id}`, { withCredentials: true });
		} catch (error) {
			message.error(error.message)
		}
	},
}