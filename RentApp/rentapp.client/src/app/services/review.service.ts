import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class ReviewService {
  private apiUrl = 'https://localhost:7020/api/reviews';

  constructor(private http: HttpClient) { }

  addReview(productId: number, review: { stars: number, comment: string }): Observable<any> {
    const headers = { Authorization: 'Bearer ' + localStorage.getItem('token') };
    return this.http.post(`${this.apiUrl}/${productId}`, review, { headers });
  }

  /** NOU: Verifică dacă userul a dat deja review la acest produs */
  hasUserReviewed(productId: number): Observable<boolean> {
    // Poți salva userName-ul logat în localStorage la autentificare!
    const userName = localStorage.getItem('userName') || '';
    return this.http.get<any[]>(`${this.apiUrl}/product/${productId}`).pipe(
      map(reviews => reviews.some(r => r.userName === userName))
    );
  }
}
