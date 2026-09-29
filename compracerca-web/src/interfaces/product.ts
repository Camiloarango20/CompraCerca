export interface Product {
    id: number;
    title: string;
    description: string;
    price: number;
    city: string;
    categoryId: number;
    categoryName: string;
    isActive: boolean;
    createdAt: string;
}

export interface PagedProductResponse {
    items: Product[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
    totalPages: number;
    hasPreviousPage: boolean;
    hasNextPage: boolean;
}