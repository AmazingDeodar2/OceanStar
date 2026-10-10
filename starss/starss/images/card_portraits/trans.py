
from pathlib import Path
from PIL import Image

# 原始图片目录
input_dir = Path(
    r"E:\塔2\Star\starss\starss\starss\images\card_portraits\big"
)

# 输出图片目录
output_dir = input_dir.parent
output_dir.mkdir(parents=True, exist_ok=True)

count = 0

for file in input_dir.glob("*.png"):
    output_file = output_dir / file.name

    with Image.open(file) as img:
        # 宽高分别缩小至 1/4
        new_width = max(1, round(img.width / 4))
        new_height = max(1, round(img.height / 4))

        # 高质量缩放
        resized = img.resize(
            (new_width, new_height),
            Image.Resampling.LANCZOS
        )

        # 保存 PNG，保留透明通道
        resized.save(output_file, format="PNG")

        print(
            f"{file.name}: "
            f"{img.width}x{img.height} -> "
            f"{new_width}x{new_height}"
        )

        count += 1

print(f"\n处理完成，共转换 {count} 张图片！")
