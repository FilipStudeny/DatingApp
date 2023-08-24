import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeaders } from './helpers/PaginationHelper';
import { Message } from '../Models/Message';


@Injectable({
  providedIn: 'root'
})
export class MessageService {
    baseURL: string = environment.apiURL;


    constructor(private http: HttpClient, ) { 

    }

    getMessages(pageNumber: number, pageSize: number, container: string) {
        let params = getPaginationHeaders(pageNumber, pageSize);
        params = params.append('Container', container);
        return getPaginatedResult<Message[]>(this.baseURL + 'messages', params, this.http);
    }

    getMessageThread(username: string){
        return this.http.get<Message[]>(this.baseURL + 'messages/thread/' + username);
    }

    sendMessage(recipientUsername: string, content: string){
        return this.http.post<Message>(this.baseURL + 'messages', {recipientUsername, content });
    }

    deleteMessage(id: number){
        return this.http.delete(this.baseURL + 'messages/' + id );
    }
}
