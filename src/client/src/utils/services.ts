import axios from "axios";
import { ConfigService } from "../services/ConfigService";

const configService = new ConfigService();
const apiBaseUrl = configService.get<string>("API_BASE_URL", "http://localhost:5062");

export const getUserData = async () => {
  const response = await axios.get(`${apiBaseUrl}/api/user`);
  console.log(response);
  return response;
};

export default {
  getUserData,
};
