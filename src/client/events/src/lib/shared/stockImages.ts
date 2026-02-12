// Stock images for event covers - all from Unsplash (free to use)
export const stockImages = [
  {
    id: 'default',
    name: 'Surreal Desert (Default)',
    url: './placeHolder.jpg',
    credit: 'Original'
  },
  {
    id: 'sunset',
    name: 'Sunset Event',
    url: 'https://images.unsplash.com/photo-1506157786151-b8491531f063?w=800&q=80',
    credit: 'Unsplash'
  }
] as const;

export type StockImage = typeof stockImages[number];

export function getImageById(id: string): StockImage {
  return stockImages.find(img => img.id === id) ?? stockImages[0];
}

export function getImageUrl(idOrUrl: string): string {
  // If it looks like a URL, return as-is
  if (idOrUrl.startsWith('http') || idOrUrl.startsWith('./') || idOrUrl.startsWith('/')) {
    return idOrUrl;
  }
  // Otherwise try to find by id
  return getImageById(idOrUrl).url;
}
