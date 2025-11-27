const getUserData = async () => {
  const response = await fetch("/api/user");
  console.log(response);
  return response.json();
};

export default {
  getUserData,
};
