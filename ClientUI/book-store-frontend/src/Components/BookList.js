import React, { useEffect, useState } from "react";
import { fetchBooks } from "../Services/api";// 

const BookList = () => {
    const [books, setBooks] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const loadBooks = async () => {
            try {
                setLoading(true);
                const data = await fetchBooks(); // Fetch the books from the API
                setBooks(data); // Update state with the fetched books
            } catch (err) {
                setError(err);
            } finally {
                setLoading(false);
            }
        };

        loadBooks();
    }, []);

    if (loading) return <p>Loading books...</p>;
    if (error) return <p>Error loading books: {error.message}</p>;
    if (books.length === 0) return <p>No books available.</p>;

    return (
        <div>
            <h1>Available Books</h1>
            <ul>
                {books.map((book) => (
                    <li key={book.id}>
                        <h2>{book.title}</h2>
                        <p>Author: {book.author}</p>
                        <p>Pages: {book.pagesNumber}</p>
                        <p>Published: {book.publicationYear}</p>
                        <p>Price: ${book.price?.toFixed(2)}</p>
                        <p>Quantity: {book.quantity}</p>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default BookList;