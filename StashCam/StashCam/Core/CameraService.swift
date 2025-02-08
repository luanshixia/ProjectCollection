//
//  CameraService.swift
//  StashCam
//
//  Created by Yang Wang on 2/6/25.
//

import AVFoundation
import UIKit

class CameraService: NSObject, ObservableObject {
    @Published var flashMode: AVCaptureDevice.FlashMode = .off
    @Published var shouldShowAlertView = false
    @Published var shouldShowSpinner = false
    @Published var willCapturePhoto = false
    @Published var isCameraInitialized = false
    @Published var photo: Photo?
    
    let session = AVCaptureSession()
    let output = AVCapturePhotoOutput()
    let photoProcessing = DispatchQueue(label: "photo.processing")
    
    var deviceInput: AVCaptureDeviceInput?
    var alertError: AlertError = AlertError()
    
    override init() {
        super.init()
        checkPermissions()
    }
    
    func checkPermissions() {
        switch AVCaptureDevice.authorizationStatus(for: .video) {
        case .notDetermined:
            AVCaptureDevice.requestAccess(for: .video) { [weak self] granted in
                guard granted else { return }
                DispatchQueue.main.async {
                    self?.setupCamera()
                }
            }
        case .restricted:
            break
        case .denied:
            break
        case .authorized:
            setupCamera()
        @unknown default:
            break
        }
    }
    
    private func setupCamera() {
        let sessionQueue = DispatchQueue(label: "camera.session")
        
        sessionQueue.async { [weak self] in
            self?.setupDevice()
            self?.setupInputOutput()
            self?.startSession()
        }
    }
    
    private func setupDevice() {
        guard let device = AVCaptureDevice.default(.builtInWideAngleCamera,
                                                   for: .video,
                                                   position: .back) else {
            print("Failed to get camera device")
            return
        }
        
        do {
            deviceInput = try AVCaptureDeviceInput(device: device)
        } catch {
            print("Failed to create camera input: \(error.localizedDescription)")
        }
    }
    
    private func setupInputOutput() {
        guard let deviceInput = deviceInput else { return }
        
        session.beginConfiguration()
        
        if session.canAddInput(deviceInput) {
            session.addInput(deviceInput)
        }
        
        if session.canAddOutput(output) {
            session.addOutput(output)
        }
        
        session.commitConfiguration()
        
        DispatchQueue.main.async { [weak self] in
            self?.isCameraInitialized = true
        }
    }
    
    private func startSession() {
        guard !session.isRunning else { return }
        session.startRunning()
    }
    
    func stopSession() {
        guard session.isRunning else { return }
        session.stopRunning()
    }
    
    func capturePhoto(albumId: UUID) {
        shouldShowSpinner = true
        willCapturePhoto = true
        
        let settings = AVCapturePhotoSettings()
        settings.flashMode = flashMode
        
        output.capturePhoto(with: settings, delegate: self)
    }
}

extension CameraService: AVCapturePhotoCaptureDelegate {
    func photoOutput(_ output: AVCapturePhotoOutput,
                     didFinishProcessingPhoto photo: AVCapturePhoto,
                     error: Error?) {
        
        if let error = error {
            print("Error capturing photo: \(error.localizedDescription)")
            return
        }
        
        guard let imageData = photo.fileDataRepresentation() else {
            print("Error while generating image from photo capture data.")
            return
        }
        
        guard let image = UIImage(data: imageData) else {
            print("Unable to generate UIImage from image data.")
            return
        }
        
        photoProcessing.async { [weak self] in
            let newPhoto = Photo(image: image)
            DispatchQueue.main.async {
                self?.photo = newPhoto
                self?.shouldShowSpinner = false
                self?.willCapturePhoto = false
            }
        }
    }
}

struct AlertError {
    let title: String = "出错了"
    let message: String = "相机无法使用，请检查相机权限设置"
    let primaryButtonTitle = "设置"
    let secondaryButtonTitle = "好的"
}
