import { useEffect, useState } from "react";
import axios from "axios";
// import getUserData from "./utils/services.ts";
import "./App.css";
import { List, ListItem, Typography } from "@mui/material";

function App() {
  const [count, setCount] = useState(0);
  const [users, setUsers] = useState<User[]>([]);

  useEffect(() => {
    const getData = async () => {
      try {
        const response = axios
          .get<User[]>("http://localhost:5062/api/user")
          .then((response) => setUsers(response.data));
        console.log(response);
      } catch (err) {
        console.error(err);
        throw new Error("Failed to fetch users");
      }
    };
    getData();
  }, []);

  const handleButton = () => {
    setCount(count + 1);
  };

  return (
    <>
      <Typography variant="h2">Notification System</Typography>
      {users.map((user) => (
        <List>
          <ListItem>
            <Typography variant="h3">{user.name}</Typography>
            <Typography variant="h4">{user.description}</Typography>
            <button onClick={handleButton}>{count}</button>
          </ListItem>
        </List>
      ))}
    </>
  );
}

export default App;
