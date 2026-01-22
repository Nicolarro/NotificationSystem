import { useEffect, useState } from "react";
import axios from "axios";
import "./App.css";
import {
  List,
  ListItem,
  Typography,
  Button,
  CircularProgress,
} from "@mui/material";

interface User {
  id: number;
  name: string;
  email: string;
  count: number;
}

function App() {
  const [count, setCount] = useState<number>(0);
  const [users, setUsers] = useState<User[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(false);

  useEffect(() => {
    const getData = async () => {
      try {
        setIsLoading(true);
        const response = await axios.get<User[]>(
          "http://localhost:5062/api/user",
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

  const handleButton = (user: User) => () => {
    setCount(count + 1);
    alert(`Notification to ${user.name} Sent`);
  };

  return (
    <>
      {isLoading ? (
        <CircularProgress />
      ) : (
        <>
          <Typography variant="h2" className="header">
            Users List
          </Typography>
          <div className="container">
            {users.map((user: User) => (
              <List>
                <ListItem key={user.id} divider className="container-item">
                  <Typography variant="h3">{user.name}</Typography>
                  <Typography variant="h4">{user.email}</Typography>
                  <Button
                    type="submit"
                    color="success"
                    variant="contained"
                    size="large"
                    className="button"
                    onClick={handleButton(user)}
                  >
                    Send Notification
                    <br />
                    Number: {user.count}
                  </Button>
                </ListItem>
              </List>
            ))}
          </div>
        </>
      )}
    </>
  );
}

export default App;
