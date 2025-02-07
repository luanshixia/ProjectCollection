//
//  AlbumDetailView.swift
//  StashCam
//
//  Created by Yang Wang on 2/6/25.
//

import SwiftUI

struct AlbumDetailView: View {
    @StateObject private var viewModel: AlbumDetailViewModel
    @Environment(\.dismiss) private var dismiss
    
    init(album: Album) {
        _viewModel = StateObject(wrappedValue: AlbumDetailViewModel(album: album))
    }
    
    private let columns = [
        GridItem(.flexible(), spacing: 1),
        GridItem(.flexible(), spacing: 1),
        GridItem(.flexible(), spacing: 1)
    ]
    
    var body: some View {
        ScrollView {
            LazyVGrid(columns: columns, spacing: 1) {
                ForEach(viewModel.photos) { photo in
                    PhotoGridItem(
                        photo: photo,
                        isSelected: viewModel.selectedPhotos.contains(photo.id),
                        isMultiSelectMode: viewModel.isMultiSelectMode
                    )
                    .onTapGesture {
                        if viewModel.isMultiSelectMode {
                            viewModel.togglePhotoSelection(photo)
                        } else {
                            // TODO: 显示照片详情
                        }
                    }
                    .onLongPressGesture {
                        viewModel.isMultiSelectMode = true
                        viewModel.togglePhotoSelection(photo)
                    }
                }
            }
        }
        .navigationTitle(viewModel.album.name)
        .navigationBarTitleDisplayMode(.inline)
        .toolbar {
            ToolbarItem(placement: .navigationBarTrailing) {
                if viewModel.isMultiSelectMode {
                    Button {
                        viewModel.deleteSelectedPhotos()
                    } label: {
                        Image(systemName: "trash")
                    }
                } else {
                    Button {
                        viewModel.isShowingCamera = true
                    } label: {
                        Image(systemName: "camera")
                    }
                }
            }
            
            ToolbarItem(placement: .navigationBarLeading) {
                if viewModel.isMultiSelectMode {
                    Button("取消") {
                        viewModel.isMultiSelectMode = false
                        viewModel.selectedPhotos.removeAll()
                    }
                }
            }
        }
        .sheet(isPresented: $viewModel.isShowingCamera) {
            CameraView(
                isPresented: $viewModel.isShowingCamera,
                albumId: viewModel.album.id
            ) { photo in
                // 处理拍摄的照片
                viewModel.photos.append(photo)
            }
        }
    }
}

struct PhotoGridItem: View {
    let photo: Photo
    let isSelected: Bool
    let isMultiSelectMode: Bool
    
    var body: some View {
        ZStack(alignment: .topTrailing) {
            if let image = photo.image {
                Image(uiImage: image)
                    .resizable()
                    .aspectRatio(1, contentMode: .fill)
                    .clipped()
            }
            
            if isMultiSelectMode {
                ZStack {
                    Circle()
                        .fill(isSelected ? Color.blue : Color.white)
                        .frame(width: 24, height: 24)
                    
                    if isSelected {
                        Image(systemName: "checkmark")
                            .font(.caption)
                            .foregroundColor(.white)
                    }
                }
                .padding(4)
            }
        }
    }
}
