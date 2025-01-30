import sqlite3

def create_database():
    # Połączenie z bazą danych (lub utworzenie nowej, jeśli nie istnieje)
    conn = sqlite3.connect("iot1.db")
    conn.close()
    print("Database and table created successfully.")

if __name__ == "__main__":
    create_database()
