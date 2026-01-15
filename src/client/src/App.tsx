import { useEffect, useState } from "react";
import axios from "axios";
import "./App.css";
import { List, ListItem, Typography } from "@mui/material";

function App() {
  const [count, setCount] = useState(0);
  const [users, setUsers] = useState<User[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(false);

  useEffect(() => {
    const getData = async () => {
      try {
        setIsLoading(true);
        const response = await axios.get<User[]>(
          "http://localhost:5062/api/user"
        );
        setUsers(response.data);
        setIsLoading(false);
        return response;
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
      {!isLoading && (
        <>
          <Typography variant="h2">Users List</Typography>
          {users.map((user: User) => (
            <List>
              <ListItem>
                <Typography variant="h3">{user.name}</Typography>
                <Typography variant="h4">{user.description}</Typography>
                <button onClick={handleButton}>{count}</button>
              </ListItem>
            </List>
          ))}
        </>
      )}
    </>
  );
}

export default App;
