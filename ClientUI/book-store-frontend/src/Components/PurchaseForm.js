import React from "react";
import { enlistPurchase } from "../Services/api";

const PurchaseForm = ({ book, onSuccess }) => {
  const handlePurchase = () => {
    const quantity = 1; // For simplicity, always purchase 1 book

    enlistPurchase(book.Id, quantity)
      .then(() => {
        alert("Purchase successful!");
        onSuccess();
      })
      .catch((error) => {
        console.error("Error during purchase:", error);
        alert("Purchase failed.");
      });
  };

  return (
    <div>
      <h3>Purchase {book.Title}</h3>
      <p>
        Author: {book.Author} - ${book.Price}
      </p>
      <button onClick={handlePurchase}>Confirm Purchase</button>
    </div>
  );
};

export default PurchaseForm;
