const token = localStorage.getItem("token");

if (!token) {
    window.location.href = "/login.html";
}

function logout() {
    localStorage.removeItem("token");
    window.location.href = "/login.html";
}

async function loadUsers() {
    try {
        const res = await fetch("/api/admin/users", {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (res.status === 401) {
            alert("Oturum geçersiz veya süresi dolmuş. Lütfen tekrar giriş yapın.");
            localStorage.removeItem("token");
            window.location.href = "/login.html";
            return;
        }

        if (res.status === 403) {
            alert("Bu sayfaya erişim yetkiniz yok.");
            return;
        }

        if (!res.ok) {
            alert("Beklenmeyen bir hata oluştu.");
            return;
        }

        const users = await res.json();
        console.log("Admin Users:", users);

        const tbody = document.getElementById("usersBody");
        tbody.innerHTML = "";

        users.forEach(user => {
            const row = document.createElement("tr");

            row.innerHTML = `
                <td>${user.id ?? ""}</td>
                <td>${user.fullName ?? ""}</td>
                <td>${user.email ?? ""}</td>
                <td>${Array.isArray(user.roles) ? user.roles.join(", ") : ""}</td>
                <td>${Array.isArray(user.permissions) ? user.permissions.join(", ") : ""}</td>
            `;

            tbody.appendChild(row);
        });

    } catch (error) {
        console.error(error);
        alert("API erişim hatası.");
    }
}

loadUsers();