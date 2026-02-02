const meEl = document.getElementById("me");
const btnLogout = document.getElementById("btnLogout");

btnLogout.addEventListener("click", () => {
    localStorage.removeItem("token");
    window.location.replace("/login.html"); // tutarlı yönlendirme
});

(async function loadMe() {
    const token = localStorage.getItem("token");
    if (!token) {
        window.location.replace("/login.html");
        return;
    }

    try {
        const res = await fetch("/api/secure/me", {
            headers: { "Authorization": `Bearer ${token}` }
        });

        if (res.status === 401) {
            localStorage.removeItem("token");
            window.location.replace("/login.html");
            return;
        }

        if (res.status === 403) {
            // token geçerli ama yetki yok: kullanıcıyı dışarı atma
            meEl.textContent = "403: Yetki yok (role/permission).";
            return;
        }

        if (!res.ok) {
            // 404/500 vb.
            meEl.textContent = `Hata: ${res.status} (${res.statusText})`;
            return;
        }

        const data = await res.json();
        meEl.textContent = `User: ${data.email ?? ""} | FullName: ${data.fullName ?? ""}`;
    } catch (e) {
        meEl.textContent = "Network error / API not reachable.";
    }
})();
