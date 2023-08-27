import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/Models/Member';
import { MembersService } from 'src/app/Services/members.service';
import { MemberMesssagesComponent } from '../member-messsages/member-messsages.component';
import { MessageService } from 'src/app/Services/message.service';
import { Message } from 'src/app/Models/Message';
import { PresenceService } from 'src/app/Services/chat/presence.service';
import { AccountService } from 'src/app/Services/account.service';
import { User } from 'src/app/Models/User';
import { take } from 'rxjs';

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
export class MemberDetailComponent implements OnInit, OnDestroy{
    member: Member = {} as Member;
    images: GalleryItem[] = [];
    messages: Message[] = [];

    @ViewChild('membersTabs', {static: true}) memberTabs?: TabsetComponent;
    activeTab?: TabDirective;

    user?: User;
    
    constructor(private accountService: AccountService, private messageService: MessageService ,
        private route: ActivatedRoute,  public presenceService: PresenceService) { 
        this.accountService.currentUser.pipe(take(1)).subscribe({
            next: user => {
                if(user) this.user = user
            }
        })

    }


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

    ngOnDestroy(): void {
        this.messageService.stopHubConnection();
    }

    selectTab(heading: string){
        if(this.memberTabs){
            this.memberTabs.tabs.find(x => x.heading === heading)!.active = true;
        }
    }

    onTabActivated(data: TabDirective){
        this.activeTab = data;
        if(this.activeTab.heading === 'Messages' && this.user){
            this.messageService.createHubConnection(this.user, this.member.username);
        }else{
            this.messageService.stopHubConnection();
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
