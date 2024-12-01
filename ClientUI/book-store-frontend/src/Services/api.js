import axios from "axios";

const API_BASE_URL = "http://localhost:8700";

export async function fetchBooks (){
    try{
        const response=axios.get(`${API_BASE_URL}/Bookstores/ListAvailableItems`);
        return response.data;
    } catch(error)
    {
console.error("Error fetching books");
throw error;
    }

  
}
export const fetchClients = () =>
  axios.get(`${API_BASE_URL}/Banks/ListClients`);

export const enlistPurchase = (bookId, count) =>
  axios.post(`${API_BASE_URL}/Bookstores/enlistPurchase`, {
    bookId,
    count,
  });

export const getBalance = (userId) =>
  axios.get(`${API_BASE_URL}/validation/getBalance/${userId}`);
