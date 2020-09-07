import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { Photo } from 'src/app/_models/photo';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { environment } from '../../../environments/environment';


@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {

  @Input() photos: Photo[];
  @Output() getMemberPhoto = new EventEmitter<string>();
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  response: string;
  currentMain: Photo;

   constructor(private alertify: AlertifyService, private authService: AuthService, private userService: UserService) { }

  ngOnInit() {
    this.initailizeUploader
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initailizeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.decodedtoken.nameid + 'photos',
      authToken: 'Bearer' + localStorage.getItem('token'),
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
   //   disableMultipart: true, // 'DisableMultipart' must be 'true' for formatDataFunction to be called.
    //  formatDataFunctionIsAsync: true,
      // formatDataFunction: async (item) => {
      // return new Promise( (resolve, reject) => {
      //    resolve({
        //    name: item._file.name,
          //  length: item._file.size,
            // contentType: item._file.type,
            // date: new Date()
          // });
        // });
      // }
    });
    
    // this.response = '';

    // this.uploader.response.subscribe( res => this.response = res );

    this.uploader.onAfterAddingFile = (file) => {file.withCredentials = false; };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if(response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMainPhoto: res.isMainPhoto
        }; 
        this.photos.push(photo);
      }
    };
  }
   setMainPhoto(photo: Photo){
     this.userService.setMainPhoto(this.authService.decodedtoken.nameid, photo.id).subscribe(
       () => {
         this.currentMain = this.photos.filter(x => x.isMainPhoto === true)[0];
         this.currentMain.isMainPhoto = false;
         photo.isMainPhoto = true;
         this.authService.changeMemberPhoto(photo.url);
         this.authService.currentUser.photoUrl = photo.url;
         localStorage.setItem('user', JSON.stringify(this.authService.currentUser))
        }, error => {
         this.alertify.error(error);
       }
     )
   }
  
}
