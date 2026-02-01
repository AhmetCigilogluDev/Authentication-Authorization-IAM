// login ekrenı tönlendirmesi , authorization kısmı
const meEl = document.getElementById("me");
const btnLogout = document.getElementById("btnLogout");

btnLogout.addEventListener("click", () => {
    localStorage.removeItem("token");
    window.location.replace("/index.html");
});

(async function loadMe() {
    const token = localStorage.getItem("token");

    const res = await fetch("/api/secure/me", {
        headers: { "Authorization": `Bearer ${token}` }
    });

    if (!res.ok) {
        // token geçersiz/expired → dışarı at
        localStorage.removeItem("token");
        window.location.replace("/index.html");
        return;
    }

    const data = await res.json();
    meEl.textContent = `User: ${data.email ?? ""} | FullName: ${data.fullName ?? ""}`;
})();