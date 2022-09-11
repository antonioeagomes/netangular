import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastrModule } from 'ngx-toastr';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';
import { FileUploadModule } from 'ng2-file-upload';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ButtonsModule } from 'ngx-bootstrap/buttons';


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    NgxGalleryModule,
    FileUploadModule,
    BsDatepickerModule.forRoot(),
    BsDropdownModule.forRoot(),
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right'
    }),
    TabsModule.forRoot(),
    PaginationModule.forRoot(),
    ButtonsModule.forRoot()    
  ],
  exports: [BsDropdownModule, ButtonsModule, ToastrModule, NgxGalleryModule, FileUploadModule, BsDatepickerModule, PaginationModule, TabsModule]
})
export class SharedModule { }
