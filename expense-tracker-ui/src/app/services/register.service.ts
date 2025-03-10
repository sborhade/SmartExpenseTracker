import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class RegisterService {
  private apiUrl = 'http://localhost:5000/api/auth/register'; // Replace with your API URL

  constructor(private http: HttpClient) {}

  // Method to handle user registration
  register(username: string, password: string): Observable<any> {
    const registerData = { username, password };
    return this.http.post<any>(this.apiUrl, registerData);
  }
}
