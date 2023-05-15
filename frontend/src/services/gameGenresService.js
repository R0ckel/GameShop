import axios from "axios";
import {gameGenresApiUrl} from "../variables/connectionVariables";
import {message} from "antd";

export const GameGenresService = {
	get: async (filters) => {
		try {
			const response = await axios.get(`${gameGenresApiUrl}`, { params: filters });
			return response.data;
		} catch (error) {
			message.error(`Error occurred! Message: ${error.message}`);
		}
	},

	getCards: async () => {
		try {
			const response = await axios.get(`${gameGenresApiUrl}`);
			return response.data;
		} catch (error) {
			message.error(`Error occurred! Message: ${error.message}`);
		}
	}
}