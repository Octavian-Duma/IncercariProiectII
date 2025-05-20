export interface Review {
  id?: number;
  productId: number;
  rating: number;
  comment: string;
  createdAt?: Date;
  userName?: string;
}
