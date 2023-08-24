import { Component, OnInit } from '@angular/core';
import { Message } from '../Models/Message';
import { IPagination } from '../Models/Pagination';
import { MessageService } from '../Services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
    messages: Message[] | undefined;
    pagination: IPagination | undefined;
    container = 'Unread';
    pageNumber = 1;
    pageSize = 5;
    loading = false;


    constructor(private messageService: MessageService) {
    }

    ngOnInit(): void {
        this.loadMessages();
    }

    loadMessages(){
        this.loading = true;
        this.messageService.getMessages(this.pageNumber, this.pageSize, this.container).subscribe({
            next: response => {
                this.messages = response.result;
                this.pagination = response.pagination
                this.loading = false;

            }
        })
    }
    
    deleteMessage(id: number){
        this.messageService.deleteMessage(id).subscribe({
            next: () => {
                this.messages?.splice(this.messages.findIndex(message => message.id === id, 1))
            }
        })
    }

    pageChanged(event: any){
        if(this.pageNumber !== event.page){
            this.pageNumber = event.page;
            this.loadMessages();
        }
    }
}
