<template>
  <div class="app">
    <header class="header">
      <div class="header-content">
        <div>
          <h1>🎵 Album Collection</h1>
          <p>Discover amazing music albums</p>
        </div>
        <button class="cart-btn" aria-label="Cart" @click="cartOpen = !cartOpen">
          🛒 Cart
          <span v-if="totalItems" class="cart-count">{{ totalItems }}</span>
        </button>
      </div>
    </header>

    <!-- Cart sidebar -->
    <div v-if="cartOpen" class="cart-sidebar" data-testid="cart-sidebar">
      <div class="cart-header">
        <h2>Your Cart</h2>
        <button class="close-btn" @click="cartOpen = false">✕</button>
      </div>
      <div v-if="cart.length === 0" class="cart-empty">Your cart is empty.</div>
      <ul v-else class="cart-items">
        <li v-for="item in cart" :key="item.album.id" class="cart-item" :data-testid="'cart-item-' + item.album.id">
          <span class="cart-item-title">{{ item.album.title }}</span>
          <div class="cart-item-right">
            <div class="qty-stepper">
              <button class="qty-btn" @click="decrementCart(item.album.id)">−</button>
              <span class="cart-item-qty">{{ item.quantity }}</span>
              <button class="qty-btn" @click="addToCart(item.album)">+</button>
            </div>
            <span class="cart-item-price">${{ (item.album.price * item.quantity).toFixed(2) }}</span>
            <button class="remove-btn" @click="removeFromCart(item.album.id)">✕</button>
          </div>
        </li>
      </ul>
      <div v-if="cart.length" class="cart-total">
        Total: ${{ cartTotal.toFixed(2) }}
      </div>
    </div>

    <main class="main">
      <div v-if="loading" class="loading">
        <div class="spinner"></div>
        <p>Loading albums...</p>
      </div>

      <div v-else-if="error" class="error">
        <p>{{ error }}</p>
        <button @click="fetchAlbums" class="retry-btn">Try Again</button>
      </div>

      <div v-else class="albums-grid">
        <AlbumCard 
          v-for="album in albums" 
          :key="album.id" 
          :album="album"
          :in-cart="isInCart(album)"
          @add-to-cart="addToCart"
        />
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import axios from 'axios'
import AlbumCard from './components/AlbumCard.vue'
import type { Album } from './types/album'

interface CartItem {
  album: Album
  quantity: number
}

const albums = ref<Album[]>([])
const loading = ref<boolean>(true)
const error = ref<string | null>(null)
const cart = ref<CartItem[]>([])
const cartOpen = ref<boolean>(false)

const totalItems = computed(() => cart.value.reduce((sum, i) => sum + i.quantity, 0))
const cartTotal = computed(() => cart.value.reduce((sum, i) => sum + i.album.price * i.quantity, 0))

const addToCart = (album: Album): void => {
  const existing = cart.value.find(i => i.album.id === album.id)
  if (existing) {
    existing.quantity++
  } else {
    cart.value.push({ album, quantity: 1 })
  }
}

const removeFromCart = (albumId: number): void => {
  cart.value = cart.value.filter(i => i.album.id !== albumId)
}

const decrementCart = (albumId: number): void => {
  const item = cart.value.find(i => i.album.id === albumId)
  if (!item) return
  if (item.quantity > 1) {
    item.quantity--
  } else {
    removeFromCart(albumId)
  }
}

const isInCart = (album: Album): boolean => cart.value.some(i => i.album.id === album.id)

const fetchAlbums = async (): Promise<void> => {
  try {
    loading.value = true
    error.value = null
    const response = await axios.get<Album[]>('/albums')
    albums.value = response.data
  } catch (err) {
    error.value = 'Failed to load albums. Please make sure the API is running.'
    console.error('Error fetching albums:', err)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchAlbums()
})
</script>

<style scoped>
.app {
  min-height: 100vh;
  padding: 2rem;
}

.header {
  text-align: center;
  margin-bottom: 3rem;
  color: white;
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  max-width: 1200px;
  margin: 0 auto;
}

.cart-btn {
  position: relative;
  background: rgba(255,255,255,0.2);
  border: 2px solid white;
  color: white;
  font-size: 1.1rem;
  padding: 0.6rem 1.2rem;
  border-radius: 25px;
  cursor: pointer;
  transition: background 0.2s;
}

.cart-btn:hover {
  background: rgba(255,255,255,0.35);
}

.cart-count {
  position: absolute;
  top: -8px;
  right: -8px;
  background: #e74c3c;
  color: white;
  border-radius: 50%;
  width: 22px;
  height: 22px;
  font-size: 0.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
}

.cart-sidebar {
  position: fixed;
  top: 0;
  right: 0;
  width: 340px;
  height: 100vh;
  background: white;
  box-shadow: -4px 0 20px rgba(0,0,0,0.2);
  z-index: 1000;
  display: flex;
  flex-direction: column;
  padding: 1.5rem;
  overflow-y: auto;
}

.cart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
  border-bottom: 1px solid #eee;
  padding-bottom: 0.8rem;
}

.cart-header h2 {
  margin: 0;
  font-size: 1.4rem;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.4rem;
  cursor: pointer;
  color: #555;
}

.cart-empty {
  color: #999;
  text-align: center;
  margin-top: 2rem;
}

.cart-items {
  list-style: none;
  padding: 0;
  margin: 0;
  flex: 1;
}

.cart-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 0;
  border-bottom: 1px solid #f0f0f0;
  font-size: 0.95rem;
}

.cart-item-right {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.qty-stepper {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  background: #f5f5f5;
  border-radius: 20px;
  padding: 0.15rem 0.4rem;
}

.qty-btn {
  background: none;
  border: none;
  font-size: 1.1rem;
  font-weight: bold;
  cursor: pointer;
  color: #667eea;
  line-height: 1;
  padding: 0 0.2rem;
}

.qty-btn:hover {
  color: #5a6fd8;
}

.cart-item-qty {
  color: #888;
  font-size: 0.85rem;
}

.remove-btn {
  background: none;
  border: none;
  color: #e74c3c;
  cursor: pointer;
  font-size: 0.9rem;
  padding: 0 0.2rem;
  line-height: 1;
}

.cart-item-title {
  font-weight: 500;
  color: #333;
}

.cart-item-price {
  color: #667eea;
  font-weight: bold;
}

.cart-total {
  margin-top: 1rem;
  padding-top: 0.8rem;
  border-top: 2px solid #667eea;
  font-size: 1.1rem;
  font-weight: bold;
  text-align: right;
  color: #333;
}

.main {
  max-width: 1200px;
  margin: 0 auto;
}

.loading {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem;
  color: white;
}

.spinner {
  width: 50px;
  height: 50px;
  border: 4px solid rgba(255, 255, 255, 0.3);
  border-top: 4px solid white;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.error {
  text-align: center;
  padding: 4rem;
  color: white;
}

.error p {
  font-size: 1.2rem;
  margin-bottom: 2rem;
}

.retry-btn {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border: 2px solid white;
  padding: 0.75rem 2rem;
  border-radius: 25px;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.3s ease;
}

.retry-btn:hover {
  background: white;
  color: #667eea;
}

.albums-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 2rem;
  padding: 1rem;
}

@media (max-width: 768px) {
  .app {
    padding: 1rem;
  }
  
  .header h1 {
    font-size: 2rem;
  }
  
  .albums-grid {
    grid-template-columns: 1fr;
    gap: 1rem;
  }
}
</style>
