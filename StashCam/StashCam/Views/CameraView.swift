//
//  CameraView.swift
//  StashCam
//
//  Created by Yang Wang on 2/6/25.
//

import SwiftUI
import AVFoundation

struct CameraPreviewView: UIViewRepresentable {
    let session: AVCaptureSession
    
    func makeUIView(context: Context) -> UIView {
        let view = UIView(frame: UIScreen.main.bounds)
        
        let previewLayer = AVCaptureVideoPreviewLayer(session: session)
        previewLayer.frame = view.frame
        previewLayer.videoGravity = .resizeAspectFill
        view.layer.addSublayer(previewLayer)
        
        return view
    }
    
    func updateUIView(_ uiView: UIView, context: Context) {}
}

struct CameraView: View {
    @StateObject private var cameraService = CameraService()
    @Binding var isPresented: Bool
    let albumId: UUID
    var photoTaken: ((Photo) -> Void)?
    
    var body: some View {
        ZStack {
            if cameraService.isCameraInitialized {
                CameraPreviewView(session: cameraService.session)
                    .ignoresSafeArea()
                
                VStack {
                    Spacer()
                    
                    HStack(spacing: 60) {
                        Button {
                            isPresented = false
                        } label: {
                            Image(systemName: "xmark")
                                .font(.title)
                                .foregroundColor(.white)
                        }
                        
                        Button {
                            cameraService.capturePhoto(albumId: albumId)
                        } label: {
                            Circle()
                                .fill(Color.white)
                                .frame(width: 70, height: 70)
                                .overlay(
                                    Circle()
                                        .stroke(Color.white, lineWidth: 2)
                                        .frame(width: 80, height: 80)
                                )
                        }
                        
                        Button {
                            cameraService.flashMode = cameraService.flashMode == .on ? .off : .on
                        } label: {
                            Image(systemName: cameraService.flashMode == .on ? "bolt.fill" : "bolt.slash.fill")
                                .font(.title)
                                .foregroundColor(.white)
                        }
                    }
                    .padding(.bottom, 30)
                }
            } else {
                Color.black
                ProgressView()
                    .progressViewStyle(CircularProgressViewStyle(tint: .white))
                    .scaleEffect(2)
            }
            
            if cameraService.shouldShowSpinner {
                Color.black.opacity(0.5)
                ProgressView()
                    .progressViewStyle(CircularProgressViewStyle(tint: .white))
                    .scaleEffect(2)
            }
        }
        .onChange(of: cameraService.photo) { photo in
            if let photo = photo {
                photoTaken?(photo)
                isPresented = false
            }
        }
        .alert(isPresented: $cameraService.shouldShowAlertView) {
            Alert(
                title: Text(cameraService.alertError.title),
                message: Text(cameraService.alertError.message),
                primaryButton: .default(Text(cameraService.alertError.primaryButtonTitle)) {
                    if let url = URL(string: UIApplication.openSettingsURLString) {
                        UIApplication.shared.open(url)
                    }
                },
                secondaryButton: .default(Text(cameraService.alertError.secondaryButtonTitle))
            )
        }
        .onDisappear {
            cameraService.stopSession()
        }
    }
}
