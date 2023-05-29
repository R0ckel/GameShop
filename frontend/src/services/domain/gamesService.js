import axios from "axios";
import {gameImagesApiUrl, gamesApiUrl} from "../../variables/connectionVariables";
import {message} from "antd";
import qs from "qs";

export const GamesService = {
	get: async (filters) => {
		try {
			const response = await axios.get(`${gamesApiUrl}`, {
				params: filters,
				withCredentials: true,
				paramsSerializer: params => qs.stringify(params, { arrayFormat: 'repeat' })
			});
			return response.data;
		} catch (error) {
			message.error(`Error occurred! Message: ${error.message}`);
		}
	},

	getById: async (id) => {
		try {
			const response = await axios.get(`${gamesApiUrl}/${id}`);
			return response.data;
		} catch (error) {
			message.error(`Error occurred! Message: ${error.message}`);
		}
	},

	add: async(formData) => {
		try{
			const response = await axios.post(`${gamesApiUrl}`, formData, {withCredentials: true});
			return response.data;
		} catch (error) {
			message.error(`Error occurred! Message: ${error.message}`);
		}
	},

	edit: async(formData) => {
		try{
			const response = await axios.put(`${gamesApiUrl}/${formData.id}`, formData, {withCredentials: true});
			return response.data;
		} catch (error) {
			message.error(`Error occurred! Message: ${error.message}`);
		}
	},

	delete: async(id) => {
		try{
			const response = await axios.delete(`${gamesApiUrl}/${id}`, {withCredentials: true});
			return response.data;
		} catch (error) {
			message.error(`Error occurred! Message: ${error.message}`);
		}
	},

	putImage: async (id, file) => {
		const formData = new FormData();
		formData.append('file', file);
		try {
			return await axios.put(`${gameImagesApiUrl}/${id}`, formData, {
				headers: {
					'Content-Type': 'multipart/form-data',
				},
				withCredentials: true
			});
		} catch (error) {
			message.error(error.message);
		}
	},
	deleteImage: async (id) => {
		try {
			const response = await axios.delete(`${gameImagesApiUrl}/${id}`, { withCredentials: true });
			return response.data;
		} catch (error) {
			message.error(error.message)
		}
	}
}