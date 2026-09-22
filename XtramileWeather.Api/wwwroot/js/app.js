const countrySelect = document.getElementById("countrySelect");
const citySelect = document.getElementById("citySelect");

const loadWeatherButton =
    document.getElementById("loadWeatherButton");

const addFavoriteButton =
    document.getElementById("addFavoriteButton");

const weatherResult =
    document.getElementById("weatherResult");

const favoriteList =
    document.getElementById("favoriteList");

const statusMessage =
    document.getElementById("statusMessage");

document.addEventListener("DOMContentLoaded", async () => {
    bindEvents();

    await Promise.all([
        loadCountries(),
        loadFavorites()
    ]);
});

function bindEvents() {
    countrySelect.addEventListener("change", loadCities);
    citySelect.addEventListener("change", updateButtons);

    loadWeatherButton.addEventListener(
        "click",
        loadWeather);

    addFavoriteButton.addEventListener(
        "click",
        addFavorite);
}

async function apiRequest(url, options = {}) {
    const response = await fetch(url, {
        ...options,
        headers: {
            Accept: "application/json",
            ...options.headers
        }
    });

    const text = await response.text();
    let data = null;

    if (text) {
        try {
            data = JSON.parse(text);
        } catch {
            data = text;
        }
    }

    if (!response.ok) {
        const message =
            typeof data === "string"
                ? data
                : data?.message;

        throw new Error(
            message || `Request failed with status ${response.status}.`);
    }

    return data;
}

async function loadCountries() {
    try {
        const countries = await apiRequest("/api/countries");

        for (const country of countries) {
            const option = document.createElement("option");

            option.value = country.id;
            option.textContent =
                `${country.name} (${country.code})`;

            countrySelect.appendChild(option);
        }
    } catch (error) {
        showStatus(error.message, true);
    }
}

async function loadCities() {
    const countryId = countrySelect.value;

    citySelect.innerHTML =
        '<option value="">Select city</option>';

    citySelect.disabled = true;
    resetWeather();
    updateButtons();

    if (!countryId) {
        return;
    }

    try {
        const cities = await apiRequest(
            `/api/cities?countryId=${countryId}`);

        for (const city of cities) {
            const option = document.createElement("option");

            option.value = city.id;
            option.textContent = city.name;

            citySelect.appendChild(option);
        }

        citySelect.disabled = false;
    } catch (error) {
        showStatus(error.message, true);
    }
}

function updateButtons() {
    const citySelected = Boolean(citySelect.value);

    loadWeatherButton.disabled = !citySelected;
    addFavoriteButton.disabled = !citySelected;
    resetWeather();
}

async function loadWeather() {
    const cityId = citySelect.value;

    if (!cityId) {
        return;
    }

    showStatus("Loading weather...");

    try {
        const weather = await apiRequest(
            `/api/weather/${cityId}`);

        weatherResult.innerHTML = `
            <h3>
                ${escapeHtml(weather.cityName)},
                ${escapeHtml(weather.countryCode)}
            </h3>

            <div class="temperature">
                ${formatNumber(weather.temperatureCelsius)} °C
            </div>

            <p>${escapeHtml(weather.description)}</p>

            <div class="weather-details">
                <span>
                    Feels like:
                    ${formatNumber(weather.feelsLikeCelsius)} °C
                </span>

                <span>
                    Humidity:
                    ${weather.humidity}%
                </span>

                <span>
                    Minimum:
                    ${formatNumber(
            weather.minimumTemperatureCelsius)} °C
                </span>

                <span>
                    Maximum:
                    ${formatNumber(
                weather.maximumTemperatureCelsius)} °C
                </span>

                <span>
                    Wind:
                    ${formatNumber(
                    weather.windSpeedMetersPerSecond)} m/s
                </span>
            </div>
        `;

        weatherResult.classList.remove("hidden");
        showStatus("");
    } catch (error) {
        resetWeather();
        showStatus(error.message, true);
    }
}

async function addFavorite() {
    const cityId = citySelect.value;

    if (!cityId) {
        return;
    }

    try {
        await apiRequest(
            `/api/favorite-cities/${cityId}`,
            { method: "POST" });

        showStatus("City added to favorites.");
        await loadFavorites();
    } catch (error) {
        showStatus(error.message, true);
    }
}

async function loadFavorites() {
    try {
        const favorites = await apiRequest(
            "/api/favorite-cities");

        if (favorites.length === 0) {
            favoriteList.innerHTML =
                "<p>No favorite cities yet.</p>";

            return;
        }

        favoriteList.innerHTML = favorites
            .map(favorite => `
                <div class="favorite-item">
                    <div>
                        <strong>
                            ${escapeHtml(favorite.cityName)}
                        </strong>

                        <span>
                            (${escapeHtml(favorite.countryCode)})
                        </span>
                    </div>

                    <button
                        class="danger"
                        data-favorite-id="${favorite.id}">
                        Remove
                    </button>
                </div>
            `)
            .join("");

        favoriteList
            .querySelectorAll("[data-favorite-id]")
            .forEach(button => {
                button.addEventListener("click", async () => {
                    await deleteFavorite(
                        button.dataset.favoriteId);
                });
            });
    } catch (error) {
        favoriteList.innerHTML =
            `<p>${escapeHtml(error.message)}</p>`;
    }
}

async function deleteFavorite(favoriteId) {
    try {
        await apiRequest(
            `/api/favorite-cities/${favoriteId}`,
            { method: "DELETE" });

        showStatus("Favorite city removed.");
        await loadFavorites();
    } catch (error) {
        showStatus(error.message, true);
    }
}

function resetWeather() {
    weatherResult.classList.add("hidden");
    weatherResult.innerHTML = "";
}

function showStatus(message, isError = false) {
    statusMessage.textContent = message;
    statusMessage.classList.toggle("error", isError);
}

function formatNumber(value) {
    return Number(value).toFixed(1);
}

function escapeHtml(value) {
    const element = document.createElement("div");
    element.textContent = value ?? "";
    return element.innerHTML;
}