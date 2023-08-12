import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../Models/Member';

@Injectable({
  providedIn: 'root'
})
export class MembersService {

    baseURL: string = environment.apiURL;

    constructor(private http: HttpClient) { }

    getMembers(){
        return this.http.get<Member[]>(this.baseURL + 'user')
    }

    getMember(username: string){
        return this.http.get<Member>(this.baseURL + 'user/' + username)
    }
}
