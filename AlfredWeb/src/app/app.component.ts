import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

type Message = {
  role: 'user' | 'assistant';
  text: string;
  time: string;
};

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {

  text = '';
  isRecording = false;

  private mediaRecorder?: MediaRecorder;
  private audioChunks: Blob[] = [];
  private mediaStream?: MediaStream;

  messages: Message[] = [
    {
      role: 'assistant',
      text: 'Olá! Sou seu assessor financeiro. Você pode registrar uma movimentação por texto ou áudio.',
      time: 'Agora'
    },
    {
      role: 'user',
      text: 'Paguei R$ 300 ao Carlos referente à compra de material.',
      time: 'Exemplo'
    },
    {
      role: 'assistant',
      text: 'Entendi: saída de R$ 300,00 para Carlos, referente à compra de material. A integração com a planilha será adicionada na próxima etapa.',
      time: 'Exemplo'
    }
  ];

  send(): void {
    const value = this.text.trim();

    if (!value) {
      return;
    }

    this.messages.push({
      role: 'user',
      text: value,
      time: this.now()
    });

    this.text = '';

    this.messages.push({
      role: 'assistant',
      text: 'Mensagem recebida. No próximo passo, ela será enviada ao backend para interpretação financeira.',
      time: this.now()
    });
  }

  async recordAudio(): Promise<void> {
    if (this.isRecording) {
      this.stopRecording();
      return;
    }

    try {
      await this.startRecording();
    } catch (error) {
      console.error('Error accessing microphone:', error);

      this.isRecording = false;

      this.messages.push({
        role: 'assistant',
        text: 'Não foi possível acessar o microfone.',
        time: this.now()
      });
    }
  }

  private async startRecording(): Promise<void> {
    this.mediaStream = await navigator.mediaDevices.getUserMedia({
      audio: true
    });

    this.audioChunks = [];

    this.mediaRecorder = new MediaRecorder(this.mediaStream);

    this.mediaRecorder.ondataavailable = (event: BlobEvent) => {
      if (event.data.size > 0) {
        this.audioChunks.push(event.data);
      }
    };

    this.mediaRecorder.onstop = () => {
      this.handleRecording();
    };

    this.mediaRecorder.start();

    this.isRecording = true;

    console.log('Recording started');
    console.log('MIME type:', this.mediaRecorder.mimeType);
  }

  private stopRecording(): void {
    if (!this.mediaRecorder) {
      return;
    }

    if (this.mediaRecorder.state !== 'inactive') {
      this.mediaRecorder.stop();
    }

    this.isRecording = false;
  }

  private handleRecording(): void {
    const mimeType =
      this.mediaRecorder?.mimeType || 'audio/webm';

    const audioBlob = new Blob(
      this.audioChunks,
      { type: mimeType }
    );

    console.log('Recording stopped');
    console.log('MIME type:', audioBlob.type);
    console.log('Blob size:', audioBlob.size, 'bytes');

    this.messages.push({
      role: 'user',
      text: '🎤 Áudio capturado.',
      time: this.now()
    });

    this.releaseMicrophone();
  }

  private releaseMicrophone(): void {
    this.mediaStream
      ?.getTracks()
      .forEach(track => track.stop());

    this.mediaStream = undefined;
    this.mediaRecorder = undefined;
  }

  private now(): string {
    return new Date().toLocaleTimeString(
      'pt-BR',
      {
        hour: '2-digit',
        minute: '2-digit'
      }
    );
  }
}