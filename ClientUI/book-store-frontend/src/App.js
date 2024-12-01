import React, { useState } from "react";
import BookList from "./Components/BookList";
import ClientList from "./Components/ClientList";
import PurchaseForm from "./Components/PurchaseForm";

const App = () => {
  const [selectedBook, setSelectedBook] = useState(null);

  return (
    <div>
      <h1>Book Store</h1>
      <ClientList />
      {!selectedBook ? (
        <BookList onSelectBook={setSelectedBook} />
      ) : (
        <PurchaseForm
          book={selectedBook}
          onSuccess={() => setSelectedBook(null)}
        />
      )}
    </div>
  );
};

export default App;
