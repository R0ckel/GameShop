import axios from "axios";
import {gamesApiUrl} from "../variables/connectionVariables";
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
	}
}