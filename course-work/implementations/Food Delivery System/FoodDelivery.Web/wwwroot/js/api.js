const API_URL = 'https://localhost:7001';

function getToken() { return localStorage.getItem('token'); }
function getRole() { return localStorage.getItem('role'); }
function getUsername() { return localStorage.getItem('username'); }
function getUserId() { return localStorage.getItem('userId'); }
function isLoggedIn() { return !!getToken(); }

function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    localStorage.removeItem('username');
    localStorage.removeItem('userId');
    localStorage.removeItem('cart');
    window.location.href = 'login.html';
}

async function apiRequest(method, endpoint, body = null) {
    const headers = { 'Content-Type': 'application/json' };
    const token = getToken();
    if (token) headers['Authorization'] = `Bearer ${token}`;
    const options = { method, headers };
    if (body) options.body = JSON.stringify(body);
    try {
        const response = await fetch(`${API_URL}/${endpoint}`, options);
        if (response.status === 401) { logout(); return null; }
        if (response.status === 204) return true;
        const data = await response.json();
        return { ok: response.ok, status: response.status, data };
    } catch (e) { return null; }
}

async function getRestaurants(page = 1, search = '', search2 = '', sortBy = '', sortDir = 'asc') {
    let url = `api/restaurants?page=${page}&pageSize=9&sortDirection=${sortDir}`;
    if (search) url += `&search=${encodeURIComponent(search)}`;
    if (search2) url += `&search2=${encodeURIComponent(search2)}`;
    if (sortBy) url += `&sortBy=${sortBy}`;
    return await apiRequest('GET', url);
}

async function getRestaurant(id) { return await apiRequest('GET', `api/restaurants/${id}`); }
async function createRestaurant(dto) { return await apiRequest('POST', 'api/restaurants', dto); }
async function updateRestaurant(id, dto) { return await apiRequest('PUT', `api/restaurants/${id}`, dto); }
async function deleteRestaurant(id) { return await apiRequest('DELETE', `api/restaurants/${id}`); }

async function uploadRestaurantImage(id, file) {
    const formData = new FormData();
    formData.append('file', file);
    const response = await fetch(`${API_URL}/api/restaurants/${id}/upload-image`, {
        method: 'POST',
        headers: { 'Authorization': `Bearer ${getToken()}` },
        body: formData
    });
    return response.ok;
}

async function getMenuItems(restaurantId = null, page = 1, search = '', search2 = '', sortBy = '', sortDir = 'asc') {
    let url = `api/menuitems?page=${page}&pageSize=12&sortDirection=${sortDir}`;
    if (restaurantId) url += `&restaurantId=${restaurantId}`;
    if (search) url += `&search=${encodeURIComponent(search)}`;
    if (search2) url += `&search2=${encodeURIComponent(search2)}`;
    if (sortBy) url += `&sortBy=${sortBy}`;
    return await apiRequest('GET', url);
}

async function getMenuItem(id) { return await apiRequest('GET', `api/menuitems/${id}`); }
async function createMenuItem(dto) { return await apiRequest('POST', 'api/menuitems', dto); }
async function updateMenuItem(id, dto) { return await apiRequest('PUT', `api/menuitems/${id}`, dto); }
async function deleteMenuItem(id) { return await apiRequest('DELETE', `api/menuitems/${id}`); }

async function uploadMenuItemImage(id, file) {
    const formData = new FormData();
    formData.append('file', file);
    const response = await fetch(`${API_URL}/api/menuitems/${id}/upload-image`, {
        method: 'POST',
        headers: { 'Authorization': `Bearer ${getToken()}` },
        body: formData
    });
    return response.ok;
}

async function getOrders(page = 1, search = '', search2 = '') {
    let url = `api/orders?page=${page}&pageSize=10&sortDirection=desc`;
    if (search) url += `&search=${encodeURIComponent(search)}`;
    if (search2) url += `&search2=${encodeURIComponent(search2)}`;
    return await apiRequest('GET', url);
}

async function getOrder(id) { return await apiRequest('GET', `api/orders/${id}`); }
async function createOrder(dto) { return await apiRequest('POST', 'api/orders', dto); }
async function updateOrderStatus(id, status) { return await apiRequest('PATCH', `api/orders/${id}/status`, { status }); }
async function deleteOrder(id) { return await apiRequest('DELETE', `api/orders/${id}`); }

async function getUsers(page = 1, search = '', search2 = '') {
    let url = `api/users?page=${page}&pageSize=100`;
    if (search) url += `&search=${encodeURIComponent(search)}`;
    if (search2) url += `&search2=${encodeURIComponent(search2)}`;
    return await apiRequest('GET', url);
}

async function updateUser(id, dto) { return await apiRequest('PUT', `api/users/${id}`, dto); }
async function deleteUser(id) { return await apiRequest('DELETE', `api/users/${id}`); }

function getCart() { return JSON.parse(localStorage.getItem('cart') || '{"restaurantId":null,"items":[]}'); }
function saveCart(cart) { localStorage.setItem('cart', JSON.stringify(cart)); }

function addToCart(menuItem, restaurantId) {
    let cart = getCart();
    if (cart.restaurantId && cart.restaurantId !== restaurantId) {
        if (!confirm('Имате артикули от друг ресторант. Да изчистим количката?')) return false;
        cart = { restaurantId: null, items: [] };
    }
    cart.restaurantId = restaurantId;
    const existing = cart.items.find(i => i.id === menuItem.id);
    if (existing) existing.qty++;
    else cart.items.push({ id: menuItem.id, name: menuItem.name, price: menuItem.price, qty: 1 });
    saveCart(cart);
    updateCartBadge();
    return true;
}

function removeFromCart(itemId) {
    let cart = getCart();
    const item = cart.items.find(i => i.id === itemId);
    if (item) {
        if (item.qty > 1) item.qty--;
        else cart.items = cart.items.filter(i => i.id !== itemId);
    }
    if (cart.items.length === 0) cart.restaurantId = null;
    saveCart(cart);
    updateCartBadge();
}

function clearCart() { saveCart({ restaurantId: null, items: [] }); updateCartBadge(); }
function getCartTotal() { return getCart().items.reduce((s, i) => s + i.price * i.qty, 0); }
function getCartCount() { return getCart().items.reduce((s, i) => s + i.qty, 0); }

function updateCartBadge() {
    const badge = document.getElementById('cart-count');
    if (badge) {
        const count = getCartCount();
        badge.textContent = count;
        badge.style.display = count > 0 ? 'flex' : 'none';
    }
}

function showAlert(message, type = 'success') {
    const div = document.createElement('div');
    div.className = `alert alert-${type} alert-dismissible fade show`;
    div.style.cssText = 'font-size:1.3rem;';
    div.innerHTML = `${message}<button type="button" class="btn-close" data-bs-dismiss="alert"></button>`;
    const container = document.getElementById('alerts');
    if (container) container.prepend(div);
    setTimeout(() => div.remove(), 4000);
}

function requireAuth() {
    if (!isLoggedIn()) { window.location.href = 'login.html'; return false; }
    return true;
}