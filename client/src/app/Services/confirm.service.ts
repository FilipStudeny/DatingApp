import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ConfirmDialogueComponent } from '../modals/confirm-dialogue/confirm-dialogue.component';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
    modalRef?: BsModalRef<ConfirmDialogueComponent>;

    constructor(private modalService: BsModalService) { }


    confirm(title = 'Confirmation', message = 'Are you sure you want to do this ?', btnOkText = 'Ok', btnCancelText = 'Cancel'): Observable<boolean>{
        const config = {
            initialState: {
                title, message, btnOkText, btnCancelText
            }
        }
        this.modalRef = this.modalService.show(ConfirmDialogueComponent, config);
        return this.modalRef.onHidden!.pipe(
            map(() => {
                return this.modalRef!.content!.result
            })
        )
    }

}
