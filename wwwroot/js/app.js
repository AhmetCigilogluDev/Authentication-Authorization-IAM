// ==============================
// DOM BINDINGS
// ==============================
const form = document.getElementById("registerForm");

const fullName = document.getElementById("fullName");
const email = document.getElementById("email");
const password = document.getElementById("password");

const btnRegister = document.getElementById("btnRegister");

const fullNameHint = document.getElementById("fullNameHint");
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

// API endpoint (aynı host üzerinde çalışıyorsan relative yeterli)
const REGISTER_URL = "/api/auth/register";

// ==============================
// LIVE VALIDATION
// ==============================
fullName.addEventListener("input", () => {
    const v = fullName.value.trim();
    if (!v) return clearError(fullName, fullNameHint);
    if (v.length < 2) return setError(fullName, fullNameHint, "Full Name en az 2 karakter olmalı.");
    clearError(fullName, fullNameHint);
});

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
// SUBMIT PIPELINE (UI -> DTO -> API -> AuthResponse -> UI)
// ==============================
form.addEventListener("submit", async (e) => {
    e.preventDefault();
    result.textContent = "";

    const fullNameVal = fullName.value.trim();
    const emailVal = email.value.trim();
    const passVal = password.value.trim();

    let ok = true;

    // FullName
    if (!fullNameVal) { setError(fullName, fullNameHint, "Full Name zorunlu."); ok = false; }
    else if (fullNameVal.length < 2) { setError(fullName, fullNameHint, "Full Name en az 2 karakter olmalı."); ok = false; }
    else clearError(fullName, fullNameHint);

    // Email
    if (!emailVal) { setError(email, emailHint, "E-mail zorunlu."); ok = false; }
    else if (!isValidEmail(emailVal)) { setError(email, emailHint, "Geçerli bir e-mail girin."); ok = false; }
    else clearError(email, emailHint);

    // Password
    if (!passVal) { setError(password, passwordHint, "Şifre zorunlu."); ok = false; }
    else if (passVal.length < 6) { setError(password, passwordHint, "Şifre en az 6 karakter olmalı."); ok = false; }
    else clearError(password, passwordHint);

    if (!ok) return;

    // DTO (UI -> API Contract)
    const payload = {
        fullName: fullNameVal,
        email: emailVal,
        password: passVal
    };

    try {
        btnRegister.disabled = true;
        btnRegister.textContent = "Registering...";

        const res = await fetch(REGISTER_URL, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        // AuthResponse bekliyoruz: { success, message, token }
        const data = await res.json().catch(() => ({}));

        if (!res.ok) {
            // API BadRequest ise message döndürür
            result.textContent = data.message ?? "Register failed.";
            return;
        }

        // success/message/token
        result.textContent = data.message ?? "Register successful.";

        if (data.token) {
            localStorage.setItem("token", data.token);
            window.location.replace("/dashboard.html");
            return;
        }

        // opsiyonel: login sayfasına yönlendir
        // window.location.href = "/index.html";

    } catch (err) {
        result.textContent = "Network error / API not reachable.";
    } finally {
        btnRegister.disabled = false;
        btnRegister.textContent = "Register";
    }
});