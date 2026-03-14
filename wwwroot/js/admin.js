var token = localStorage.getItem("token");

if (!token) {
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

        //getting the responsed data from the api
        const users = await res.json();

        console.log("Admin Users:", users);

        // JSON data is rendering on the user interface
        const table = document.getElementById("usersTable");
        table.innerHTML = "";

        users.forEach(user => {

            const row = `
                < tr >
                <td>${user.fullName}</td>
                <td>${user.email}</td>
                <td>${user.roles.join(", ")}</td>
                <td>${user.permissions.join(", ")}</td>
            </tr >


            `;

            table.innerHTML += row;


        });

    } catch (error) {
        console.error(error);
        alert("API erişim hatası.");
    }
}

loadUsers();
