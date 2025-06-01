# 🖧 Simple TCP Web Server in C# (.NET)

This is a basic multi-threaded web server built using raw TCP sockets in C# and .NET. It serves static HTML, CSS, and JavaScript files from a specified `webroot/` directory, handles HTTP `GET` requests, and returns appropriate HTTP responses for missing files, unsupported methods, and invalid file types.

---

## 📁 Folder Structure

 - project-root/
 - ├── sockets/
 - │ ├── Program.cs # Main server logic
 - │ ├── webroot/ # Static file directory
 - │ │ ├── index.html
 - │ │ ├── about.html
 - │ │ ├── styles.css
 - │ │ ├── script.js
 - │ │ └── error.html
 - │ └── server.log # Request log file (auto-generated)

---

## 🚀 Features

- 🔌 Handles raw TCP socket connections on port `8080`
- 🌐 Responds to valid HTTP `GET` requests with static files
- 📄 Supports `.html`, `.css`, and `.js` MIME types
- 🧠 Multi-threaded: Each client request is handled in a new thread
- 📁 Dynamically locates the `webroot/` folder up the directory tree
- ❌ Handles errors with proper HTTP codes:
  - `404 Not Found` for missing files
    ![notfound](https://github.com/user-attachments/assets/0edd4990-fae4-49f6-9bda-262cd77f5049)
  - `405 Method Not Allowed` for unsupported HTTP methods
  - `403 Forbidden` for invalid file types or directory traversal attempts
    ![forbidden](https://github.com/user-attachments/assets/4f767bb2-ee78-41dd-b81b-ebfadbfd6f7c)
- 📓 Logs all incoming requests to `server.log`
- 🛠 Custom error page (`error.html`) support using `{{error}}` placeholder

## 🧪 Sample Endpoints

- `GET /index.html` → Serves homepage
- `GET /about.html` → Serves about page
- `GET /styles.css` → Returns stylesheet with proper MIME
- `GET /nonexistent.html` → Returns 404 error

---

## 🛡 Security

- Prevents directory traversal (`../../etc/passwd`)
- Restricts access to files only within the `webroot/` directory
- Supports only `.html`, `.css`, `.js` to avoid arbitrary file access

---

## 🛠 How It Works

- The server starts by locating `webroot/` using a recursive parent path scan.
- It listens for incoming TCP connections on port `8080`.
- For each connection, it:
  1. Reads the HTTP request line
  2. Validates method, file extension, and file existence
  3. Returns content with correct headers or an error page
- After the response, the connection is closed and logged.

---

## 🧾 Example HTML (webroot/index.html)

```html
<!DOCTYPE html>
<html>
<head>
  <title>Home</title>
  <link rel="stylesheet" href="styles.css" />
</head>
<body>
  <h1>Welcome to My Web Server</h1>
  <script src="script.js"></script>
</body>
</html>
```

![index](https://github.com/user-attachments/assets/eb73b8ff-bea9-4503-82c8-c944c2fd27c5)


## Running the Server
- Follow these steps to run the TCP web server:

### 1. Clone the repository
- `git clone https://github.com/your-username/your-repo-name.git
    cd your-repo-name/sockets`
### 2. Open the project in Visual Studio (or)
####    If using .NET CLI, run:
- `dotnet run`
### 3. Make sure the 'webroot/' folder exists in the correct path:
-    `your-repo-name/sockets/webroot/`

It should contain files like:
- index.html
- about.html
- styles.css
- script.js
- error.html
### 4. Open a web browser and navigate to:
`http://localhost:8080/index.html`
