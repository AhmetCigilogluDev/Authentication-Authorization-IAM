// ==============================
// DOM BINDINGS
// ==============================
const form = document.getElementById("loginForm");

const email = document.getElementById("email");
const password = document.getElementById("password");

const btnLogin = document.getElementById("btnLogin");

const emailHint = document.getElementById("emailHint");
const passwordHint = document.getElementById("passwordHint");

const result = document.getElementById("result");

// ==============================
// HELPERS
// ==============================
function setError(inputEl, hintEl, message) {
    inputEl.classList.add("input-error");
    hintEl.classList.add("field-error");
    hintEl.textContent = message;
}

function clearError(inputEl, hintEl) {
    inputEl.classList.remove("input-error");
    hintEl.classList.remove("field-error");
    hintEl.textContent = "";
}

function isValidEmail(v) {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v);
}

const LOGIN_URL = "/api/auth/login";

// ==============================
// LIVE VALIDATION
// ==============================
email.addEventListener("input", () => {
    const v = email.value.trim();
    if (!v) return clearError(email, emailHint);
    if (!isValidEmail(v)) return setError(email, emailHint, "Geçerli bir e-mail girin.");
    clearError(email, emailHint);
});

password.addEventListener("input", () => {
    const v = password.value.trim();
    if (!v) return clearError(password, passwordHint);
    if (v.length < 6) return setError(password, passwordHint, "Şifre en az 6 karakter olmalı.");
    clearError(password, passwordHint);
});

// ==============================
// SUBMIT PIPELINE
// ==============================
form.addEventListener("submit", async (e) => {
    e.preventDefault();
    result.textContent = "";

    const emailVal = email.value.trim();
    const passVal = password.value.trim();

    let ok = true;

    if (!emailVal) { setError(email, emailHint, "E-mail zorunlu."); ok = false; }
    else if (!isValidEmail(emailVal)) { setError(email, emailHint, "Geçerli bir e-mail girin."); ok = false; }
    else clearError(email, emailHint);

    if (!passVal) { setError(password, passwordHint, "Şifre zorunlu."); ok = false; }
    else if (passVal.length < 6) { setError(password, passwordHint, "Şifre en az 6 karakter olmalı."); ok = false; }
    else clearError(password, passwordHint);

    if (!ok) return;

    // DTO (UI -> API Contract)
    const payload = {
        email: emailVal,
        password: passVal
    };

    try {
        btnLogin.disabled = true;
        btnLogin.textContent = "Logging in...";

        const res = await fetch(LOGIN_URL, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        const data = await res.json().catch(() => ({}));

        if (!res.ok) {
            result.textContent = data.message ?? "Login failed.";
            return;
        }

        result.textContent = data.message ?? "Login successful.";

        if (data.token) {
            localStorage.setItem("token", data.token);
        }



        // login ekrenı tönlendirmesi , authorization kısmı


        //routing the page in the system
        //if (data.token) {
        //    localStorage.setItem("token", data.token);
        //    window.location.href = "/dashboard.html";
        //}

        //// örnek yönlendirme
        // window.location.href = "/dashboard.html";

    } catch {
        result.textContent = "Network error / API not reachable.";
    } finally {
        btnLogin.disabled = false;
        btnLogin.textContent = "Login";
    }
});