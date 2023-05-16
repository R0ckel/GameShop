import axios from "axios";
import {commentsApiUrl} from "../../variables/connectionVariables";
import {message} from "antd";

export const CommentService  = {
	getByGameId: async (gameId, filters) => {
		try {
			const response = await axios.get(`${commentsApiUrl}`, { params: {gameId, ...filters} });
			return response.data;
		} catch (error) {
			message.error(`Error occurred! Message: ${error.message}`);
		}
	},

	post: async(comment) => {
		try{
			const response = await axios.post(`${commentsApiUrl}`, comment, { withCredentials: true });
			return response.data;
		} catch (error) {
			message.error(`Error occurred! Message: ${error.message}`);
		}
	},

	delete: async(id) => {
		try{
			const response = await axios.delete(`${commentsApiUrl}/${id}`, { withCredentials: true });
			return response.data;
		} catch (error) {
			message.error(`Error occurred! Message: ${error.message}`);
		}
	}
}