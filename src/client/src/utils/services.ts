import axios from "axios";

export const getUserData = async () => {
  const response = await axios.get("/api/user");
  console.log(response);
  return response;
};

export default {
  getUserData,
};
