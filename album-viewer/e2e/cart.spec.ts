import { test, expect } from '@playwright/test';
import path from 'path';

test('add first album to cart and verify cart contents', async ({ page }) => {
  // Step 1: Open the Album App
  await page.goto('/');
  await expect(page).toHaveTitle('Album Viewer');

  // Step 2: Click "Add to Cart" on the first album tile
  const firstAddToCartButton = page.locator('button:has-text("Add to Cart")').first();
  await expect(firstAddToCartButton).toBeVisible();
  const firstAlbumTitle = await page.locator('.album-title').first().textContent();
  await firstAddToCartButton.click();

  // Verify cart badge shows 1
  const cartBadge = page.locator('.cart-count');
  await expect(cartBadge).toHaveText('1');

  // Step 3: Click the cart button in the top right to open the cart
  await page.locator('button[aria-label="Cart"]').click();

  // Step 4: Verify the cart sidebar is visible and contains the added album
  const cartSidebar = page.locator('[data-testid="cart-sidebar"]');
  await expect(cartSidebar).toBeVisible();
  await expect(cartSidebar.locator('.cart-item-title')).toHaveText(firstAlbumTitle!.trim());
  await expect(cartSidebar.locator('.cart-total')).toContainText('$');

  // Step 5: Take a screenshot of the cart
  await page.screenshot({
    path: path.join('e2e', 'screenshots', 'cart.png'),
    fullPage: false,
  });
});
