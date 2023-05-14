import {basketApiUrl} from "../variables/connectionVariables";
import {message} from "antd";
import axios from "axios";

export const basketService = {
	getBasket: async () => {
		try {
			const response = await axios.get(`${basketApiUrl}`, { withCredentials: true });
			return response.data;
		} catch (error) {
			if (error.response && error.response.status === 401) {
				message.error("Unauthorized move has been detected!")
					.then(() => message.error("Login first!"));
			} else {
				message.error("Error occurred!")
					.then(() => message.error(`Message: ${error.message}`));
			}
		}
	}
}