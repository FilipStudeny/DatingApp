import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../Models/User';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
    //HTTP REQUESTS FOR USER ACCOUNTS
    
    apiURL: string = environment.apiURL;
    private currentUserSource = new BehaviorSubject<User | null>(null);
    currentUser = this.currentUserSource.asObservable();

    constructor(private http: HttpClient) { }

    login(model: User){
        return this.http.post<User>(this.apiURL + 'account/login', model).pipe(
            map((response: User) => {
                const user: User = response;
                if(user){
                    this.setCurrentUser(user);
                }
            })
        );
    }

    register(model: any){
        return this.http.post<User>(this.apiURL + 'account/register', model).pipe(
            map(user => {
                if(user){
                    this.setCurrentUser(user);
                }
            })
        );
    }

    setCurrentUser(user: User){
        localStorage.setItem('user', JSON.stringify(user));
        this.currentUserSource.next(user);
    }

    loggout(){
        localStorage.removeItem('user');
        this.currentUserSource.next(null);
    }


}
