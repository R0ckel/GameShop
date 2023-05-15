import axios from "axios";
import {companiesApiUrl} from "../variables/connectionVariables";
import {message} from "antd";

export const CompaniesService = {
	get: async (filters) => {
		try {
			const response = await axios.get(`${companiesApiUrl}`, { params: filters });
			return response.data;
		} catch (error) {
			message.error(`Error occurred! Message: ${error.message}`);
		}
	},

	getCards: async () => {
		try {
			const response = await axios.get(`${companiesApiUrl}`);
			return response.data;
		} catch (error) {
			message.error(`Error occurred! Message: ${error.message}`);
		}
	}
}