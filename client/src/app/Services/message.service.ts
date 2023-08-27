import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeaders } from './helpers/PaginationHelper';
import { Message } from '../Models/Message';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../Models/User';
import { BehaviorSubject, take } from 'rxjs';
import { Group } from '../Models/Group';


@Injectable({
  providedIn: 'root'
})
export class MessageService {
    baseURL: string = environment.apiURL;
    chatURL: string = environment.chatURL;

    private hubConnection?: HubConnection;
    private messageThreadSouce = new BehaviorSubject<Message[]>([]);
    messageThread = this.messageThreadSouce.asObservable();


    constructor(private http: HttpClient, ) { 
    }

    createHubConnection(user: User, otherUsername: string){
        this.hubConnection = new HubConnectionBuilder().withUrl(this.chatURL + 'message?user=' + otherUsername, {
            accessTokenFactory: () => user.token
        }).withAutomaticReconnect().build();

        this.hubConnection.start().catch(error => console.error(error));
        
        this.hubConnection.on('RecieveMessageThread', messages => {
            console.log(messages);
            this.messageThreadSouce.next(messages);
        })

        this.hubConnection.on("UpdatedGroup", (group: Group) => {
            if(group.connections.some(connection => connection.username === otherUsername)){
                this.messageThread.pipe(take(1)).subscribe({
                    next: messages => {
                        messages.forEach(message => {
                            if(!message.dateRead){
                                message.dateRead = new Date(Date.now());
                            }
                        })

                        this.messageThreadSouce.next([...messages]);
                    }
                })
            }
        })

        this.hubConnection.on('NewMessage', message => {
            this.messageThread.pipe(take(1)).subscribe({
                next: messages => {
                    console.log(messages);
                    this.messageThreadSouce.next([...messages, message]);
                }
            });
        })
    }

    stopHubConnection(){
        if(this.hubConnection){
            this.hubConnection?.stop();
        }
    }

    getMessages(pageNumber: number, pageSize: number, container: string) {
        let params = getPaginationHeaders(pageNumber, pageSize);
        params = params.append('Container', container);
        return getPaginatedResult<Message[]>(this.baseURL + 'messages', params, this.http);
    }

    getMessageThread(username: string){
        return this.http.get<Message[]>(this.baseURL + 'messages/thread/' + username);
    }

    async sendMessage(recipientUsername: string, content: string){
        return this.hubConnection?.invoke("SendMessage", {recipientUsername, content})
            .catch(error => console.error(error));
    }

    deleteMessage(id: number){
        return this.http.delete(this.baseURL + 'messages/' + id );
    }
}
