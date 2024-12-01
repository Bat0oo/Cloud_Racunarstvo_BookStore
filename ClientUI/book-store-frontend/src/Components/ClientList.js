import React, { useEffect, useState } from "react";
import { fetchClients } from "../Services/api";

const ClientList = () => {
  const [clients, setClients] = useState([]);

  useEffect(() => {
    fetchClients()
      .then((response) => {
        setClients(response.data);
      })
      .catch((error) => console.error("Error fetching clients:", error));
  }, []);

  return (
    <div>
      <h2>Clients</h2>
      <ul>
        {clients.map((client) => (
          <li key={client.Id}>
            {client.FirstName} {client.LastName} - Balance: ${client.BankAccount}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default ClientList;
