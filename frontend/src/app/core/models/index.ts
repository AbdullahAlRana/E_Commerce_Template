export interface UserInfo { id: string; email: string; firstName: string; lastName: string; phone?: string; roles: string[]; }
export interface AuthResponse { accessToken: string; refreshToken: string; expiresAt: string; user: UserInfo; }
export interface Category { id: string; name: string; description?: string; imageUrl?: string; parentCategoryId?: string; isActive: boolean; productCount: number; }
export interface Brand { id: string; name: string; description?: string; logoUrl?: string; isActive: boolean; }
export interface ProductImage { id: string; url: string; isPrimary: boolean; sortOrder: number; }
export interface Product { id: string; name: string; description?: string; sku: string; price: number; discountPrice?: number; effectivePrice: number; stockQuantity: number; categoryId: string; categoryName?: string; brandId?: string; brandName?: string; isActive: boolean; isFeatured: boolean; averageRating: number; reviewCount: number; images: ProductImage[]; primaryImageUrl?: string; createdAt: string; }
export interface PagedResult<T> { items: T[]; page: number; pageSize: number; totalCount: number; totalPages: number; }
export interface CartItem { id: string; productId: string; productName: string; productImage?: string; unitPrice: number; quantity: number; total: number; stockQuantity: number; }
export interface Cart { id: string; items: CartItem[]; subTotal: number; totalItems: number; }
export interface Address { id: string; fullName: string; phone: string; street: string; city: string; state: string; zipCode: string; country: string; isDefault: boolean; }
export interface Review { id: string; productId: string; userId: string; userName: string; rating: number; comment?: string; createdAt: string; }
export interface OrderItem { id: string; productId: string; productName: string; productImage?: string; unitPrice: number; quantity: number; total: number; }
export interface Order { id: string; orderNumber: string; userId: string; customerName?: string; customerEmail?: string; status: number; paymentStatus: number; paymentMethod: number; subTotal: number; shippingCost: number; tax: number; discount: number; total: number; shippingAddressId?: string; shippingAddress?: Address; notes?: string; couponCode?: string; items: OrderItem[]; createdAt: string; }