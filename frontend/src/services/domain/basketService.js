import {basketApiUrl} from "../../variables/connectionVariables";
import {message} from "antd";
import axios from "axios";

export const BasketService = {
	getBasket: async () => {
		try {
			const response = await axios.get(`${basketApiUrl}`, { withCredentials: true });
			return response.data;
		} catch (error) {
			if (error.response && error.response.status === 401) {
				message.error("Unauthorized move has been detected! You must login first!");
			} else {
				message.error(`Error occurred! Message: ${error.message}`);
			}
		}
	},

	addItem: async (gameId) => {
		try{
			const response = await axios.post(`${basketApiUrl}/add/${gameId}`, {}, { withCredentials: true })
			return response.data;
		} catch (error) {
			if (error.response && error.response.status === 401) {
				message.error("Unauthorized move has been detected! You must login first!");
			} else {
				message.error(`Error occurred! Message: ${error.message}`);
			}
		}
	},

	removeItem: async (gameId) => {
		try{
			const response = await axios.delete(`${basketApiUrl}/remove/${gameId}`, { withCredentials: true })
			return response.data;
		} catch (error) {
			if (error.response && error.response.status === 401) {
				message.error("Unauthorized move has been detected! You must login first!");
			} else {
				message.error(`Error occurred! Message: ${error.message}`);
			}
		}
	},

	getTotal: async () => {
		try{
			const response = await axios.get(`${basketApiUrl}/total`, { withCredentials: true })
			return response.data;
		} catch (error) {
			if (error.response && error.response.status === 401) {
				message.error("Unauthorized move has been detected! You must login first!");
			} else {
				message.error(`Error occurred! Message: ${error.message}`);
			}
		}
	},

	clear: async () => {
		try{
			const response = await axios.delete(`${basketApiUrl}/clear`, { withCredentials: true })
			return response.data;
		} catch (error) {
			if (error.response && error.response.status === 401) {
				message.error("Unauthorized move has been detected! You must login first!");
			} else {
				message.error(`Error occurred! Message: ${error.message}`);
			}
		}
	}
}