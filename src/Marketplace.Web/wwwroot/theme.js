window.marketplaceTheme = (() => {
    const storageKey = "marketplace-theme";
    const root = document.documentElement;
    const query = window.matchMedia("(prefers-color-scheme: dark)");
    let blazorBound = false;
    let observerBound = false;

    function readCookieTheme() {
        const item = document.cookie
            .split("; ")
            .find((cookie) => cookie.startsWith(`${storageKey}=`));
        const value = item?.split("=")[1];

        return value === "dark" || value === "light" ? value : null;
    }

    function persistTheme(theme) {
        try {
            window.localStorage?.setItem(storageKey, theme);
        } catch {
            // Some browser/privacy contexts disable localStorage.
        }

        document.cookie = `${storageKey}=${theme}; path=/; max-age=31536000; SameSite=Lax`;
    }

    function storedTheme() {
        try {
            const saved = window.localStorage?.getItem(storageKey);
            if (saved === "dark" || saved === "light") {
                return saved;
            }
        } catch {
            // Ignore and use the cookie fallback.
        }

        return readCookieTheme();
    }

    function preferredTheme() {
        const saved = storedTheme();
        if (saved) {
            return saved;
        }

        return query.matches ? "dark" : "light";
    }

    function label(theme) {
        return theme === "dark" ? "Escuro" : "Claro";
    }

    function apply(theme, persist = false) {
        if (persist) {
            persistTheme(theme);
        }

        root.dataset.theme = theme;
        root.dataset.bsTheme = theme;
        root.style.colorScheme = theme;
        return label(theme);
    }

    function sync() {
        return apply(preferredTheme());
    }

    function bindBlazorNavigation() {
        if (blazorBound) {
            return;
        }

        if (window.Blazor?.addEventListener) {
            window.Blazor.addEventListener("enhancedload", sync);
            blazorBound = true;
            return;
        }

        window.setTimeout(bindBlazorNavigation, 50);
    }

    function bindThemeObserver() {
        if (observerBound) {
            return;
        }

        const observer = new MutationObserver(() => {
            const expected = preferredTheme();
            if (root.dataset.theme !== expected || root.dataset.bsTheme !== expected) {
                apply(expected);
            }
        });

        observer.observe(root, {
            attributes: true,
            attributeFilter: ["data-theme", "data-bs-theme"]
        });
        observerBound = true;
    }

    query.addEventListener("change", () => {
        if (!storedTheme()) {
            apply(preferredTheme());
        }
    });

    window.addEventListener("pageshow", sync);
    document.addEventListener("DOMContentLoaded", sync);
    bindBlazorNavigation();
    bindThemeObserver();

    return {
        init() {
            bindBlazorNavigation();
            bindThemeObserver();
            return sync();
        },
        toggle() {
            const next = preferredTheme() === "dark" ? "light" : "dark";
            return apply(next, true);
        },
        current() {
            return preferredTheme();
        }
    };
})();

window.marketplaceUi = {
    downloadFile(fileName, contentType, base64) {
        const link = document.createElement("a");
        const bytes = Uint8Array.from(atob(base64), (char) => char.charCodeAt(0));
        const blob = new Blob([bytes], { type: contentType || "application/octet-stream" });
        link.href = URL.createObjectURL(blob);
        link.download = fileName;
        link.click();
        URL.revokeObjectURL(link.href);
    },
    initCarousels() {
        if (!window.bootstrap) {
            return false;
        }

        document.querySelectorAll("[data-bs-ride='carousel']").forEach((element) => {
            const instance = window.bootstrap.Carousel.getOrCreateInstance(element, {
                interval: Number.parseInt(element.getAttribute("data-bs-interval") || "4500", 10),
                ride: "carousel",
                touch: true,
                wrap: true
            });
            instance.cycle();
        });

        return true;
    }
};

window.marketplaceSession = {
    async antiforgeryToken() {
        const response = await fetch("/auth/antiforgery", {
            method: "GET",
            credentials: "same-origin"
        });

        if (!response.ok) {
            return "";
        }

        try {
            const payload = await response.json();
            return payload.token || "";
        } catch {
            return "";
        }
    },
    async login(login, password) {
        const token = await this.antiforgeryToken();
        const response = await fetch("/auth/login", {
            method: "POST",
            credentials: "same-origin",
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": token
            },
            body: JSON.stringify({ login, password })
        });

        if (response.ok) {
            return { succeeded: true };
        }

        if (response.status === 401) {
            return { succeeded: false, message: "Login ou senha invalidos." };
        }

        try {
            return await response.json();
        } catch {
            return { succeeded: false, message: "Nao foi possivel entrar." };
        }
    },
    async logout() {
        const token = await this.antiforgeryToken();
        await fetch("/auth/logout", {
            method: "POST",
            credentials: "same-origin",
            headers: {
                "RequestVerificationToken": token
            }
        });
    }
};
