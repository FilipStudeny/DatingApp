import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/Models/Member';
import { MembersService } from 'src/app/Services/members.service';
import { MemberMesssagesComponent } from '../member-messsages/member-messsages.component';
import { MessageService } from 'src/app/Services/message.service';
import { Message } from 'src/app/Models/Message';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [
    CommonModule,
    TabsModule,
    GalleryModule,
    TimeagoModule,
    MemberMesssagesComponent
  ]
})
export class MemberDetailComponent implements OnInit{
    member: Member = {} as Member;
    images: GalleryItem[] = [];
    messages: Message[] = [];

    @ViewChild('membersTabs', {static: true}) memberTabs?: TabsetComponent;
    activeTab?: TabDirective;
    
    constructor(private memberService: MembersService, private messageService: MessageService ,private route: ActivatedRoute) { }

    ngOnInit(): void {
        this.route.data.subscribe({
            next: data => this.member = data['member']
        })

        this.route.queryParams.subscribe({
            next: params => {
                params['tab'] && this.selectTab(params['tab']);
            }
        })

        this.getImages();
    }

    selectTab(heading: string){
        if(this.memberTabs){
            this.memberTabs.tabs.find(x => x.heading === heading)!.active = true;
        }
    }

    onTabActivated(data: TabDirective){
        this.activeTab = data;
        if(this.activeTab.heading == 'Messages'){
            this.loadMessages();
        }
    }

    loadMessages(){
        if(this.member?.username){
            this.messageService.getMessageThread(this.member?.username).subscribe({
                next: messages => this.messages = messages
            })
        }
    }


    getImages(){
        if(!this.member) return;
        for (const photo of this.member?.photos){
            this.images.push(new ImageItem({src: photo.url, thumb: photo.url}));
        }
    }

}
