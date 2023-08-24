import { CommonModule } from '@angular/common';
import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from 'src/app/Models/Message';
import { MessageService } from 'src/app/Services/message.service';


@Component({
  selector: 'app-member-messsages',
  templateUrl: './member-messsages.component.html',
  styleUrls: ['./member-messsages.component.css'],
  standalone: true,
  imports: [
    CommonModule, TimeagoModule, FormsModule
  ]
})
export class MemberMesssagesComponent implements OnInit {
    @Input() messages: Message[] = [];
    @Input() username?: string;
    messageContent = '';
    @ViewChild('messageForm') messageForm?: NgForm;

    
    constructor(private messageService: MessageService) {
    }

    ngOnInit(): void {
    }

    sendMessage(){
    
        if(!this.username) return;
        this.messageService.sendMessage(this.username, this.messageContent).subscribe({
            next: message => {
                this.messages.push(message)
                this.messageForm?.reset();
            }
        });
    }

}
